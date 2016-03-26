using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace XMock.Runners
{
    public partial class TestAssemblyRunner : XunitTestAssemblyRunner
    {
        public TestAssemblyRunner(ITestAssembly testAssembly, IEnumerable<IXunitTestCase> testCases, IMessageSink diagnosticMessageSink, IMessageSink executionMessageSink, ITestFrameworkExecutionOptions executionOptions)
            : base(testAssembly, testCases, diagnosticMessageSink, executionMessageSink, executionOptions)
        {
        }

        protected override string GetTestFrameworkDisplayName()
        {
            return "Xunit-TypeMock Test Framework (XMock)";
        }

        protected override async Task<RunSummary> RunTestCollectionsAsync(IMessageBus messageBus, CancellationTokenSource cancellationTokenSource)
        {
            originalSyncContext = SynchronizationContext.Current;

            TestCollectionCategories testCollectionCategories;
            try
            {
                testCollectionCategories = new TestCaseProcessor(TestCases).Run(cancellationTokenSource.Token);
            }
            catch (Exception e)
            {
                DiagnosticMessageSink.OnMessage(new DiagnosticMessage($"Test case processor threw exception: {e}"));
                return new RunSummary();
            }

            var summaries = new List<RunSummary>();
            RunSummary summary;
            if (testCollectionCategories.TypemockPragmaticCollections.Count > 0)
            {
                // always run Typemock pragmatic collections synchronously
                summary = new RunSummary();
                OnTypemockPragmaticTestsStarting();
                await RunTestCollections(testCollectionCategories.TypemockPragmaticCollections, summary, messageBus, cancellationTokenSource);
                OnTypemockPragmaticTestsFinished();
                summaries.Add(summary);
            }
            if (testCollectionCategories.TypemockInterfaceOnlyCollections.Count > 0)
            {
                summary = new RunSummary();
                // always run Typemock interface-only collections synchronously
                OnTypemockInterfaceOnlyTestsStarting();
                await RunTestCollections(testCollectionCategories.TypemockInterfaceOnlyCollections, summary, messageBus, cancellationTokenSource);
                OnTypemockInterfaceOnlyTestsFinished();
                summaries.Add(summary);
            }
            if (testCollectionCategories.TypemockPragmaticCollections.Count + testCollectionCategories.TypemockInterfaceOnlyCollections.Count > 0)
            {
                OnAllTypemockTestsFinished();
            }

            if (disableParallelization) // run the remaining collections synchronously
            {
                summary = new RunSummary();
                if (testCollectionCategories.OtherCollections.Count > 0)
                {
                    OnOtherTestsStarting();
                    await RunTestCollections(testCollectionCategories.OtherCollections, summary, messageBus, cancellationTokenSource);
                    OnOtherTestsFinished();
                }
                summaries.Add(summary);
            }
            else // run the remaining collections async
            {
                SetupSyncContext(maxParallelThreads);

                Func<Func<Task<RunSummary>>, Task<RunSummary>> taskRunner;
                if (SynchronizationContext.Current != null)
                {
                    var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
                    taskRunner = code => Task.Factory.StartNew(code, cancellationTokenSource.Token, TaskCreationOptions.DenyChildAttach | TaskCreationOptions.HideScheduler, scheduler).Unwrap();
                }
                else
                    taskRunner = code => Task.Run(code, cancellationTokenSource.Token);

                OnOtherTestsStarting();
                var otherCompletedAsync = await RunTestCollectionsAsync(testCollectionCategories.OtherCollections, summaries, messageBus, cancellationTokenSource, taskRunner);
                await otherCompletedAsync;
                OnOtherTestsFinished();
            }

            return new RunSummary
            {
                Total = summaries.Sum(s => s.Total),
                Failed = summaries.Sum(s => s.Failed),
                Skipped = summaries.Sum(s => s.Skipped)
            };
        }

        private async Task RunTestCollections(IEnumerable<TestCollection> collections, RunSummary summary, IMessageBus messageBus, CancellationTokenSource cancellationTokenSource)
        {
            foreach (var collection in collections)
            {
                summary.Aggregate(await RunTestCollectionAsync(messageBus, collection.Unwrap(), collection.TestCases, cancellationTokenSource));
                if (cancellationTokenSource.IsCancellationRequested)
                {
                    break;
                }
            }
        }

        private async Task<Task> RunTestCollectionsAsync(IEnumerable<TestCollection> collections, ICollection<RunSummary> summaries, IMessageBus messageBus, CancellationTokenSource cancellationTokenSource, Func<Func<Task<RunSummary>>, Task<RunSummary>> taskRunner)
        {
            var tasks = collections
                .Select(collection => taskRunner(() => RunTestCollectionAsync(messageBus, collection.Unwrap(), collection.TestCases, cancellationTokenSource)))
                .ToArray();

            foreach (var task in tasks)
            {
                try
                {
                    summaries.Add(await task);
                }
                catch (TaskCanceledException)
                {
                }
            }
            // return a task which completes when all tasks complete
            return Task.WhenAll(tasks);
        }

        protected override Task<RunSummary> RunTestCollectionAsync(IMessageBus messageBus, ITestCollection testCollection, IEnumerable<IXunitTestCase> testCases, CancellationTokenSource cancellationTokenSource)
        {
            return new TestCollectionRunner(testCollection, testCases, DiagnosticMessageSink, messageBus, TestCaseOrderer, new ExceptionAggregator(Aggregator), cancellationTokenSource).RunAsync();
        }
    }
}