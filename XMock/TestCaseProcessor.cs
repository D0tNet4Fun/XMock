﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace XMock
{
    internal class TestCaseProcessor
    {
        private readonly IEnumerable<IXunitTestCase> _testCases;

        public TestCaseProcessor(IEnumerable<IXunitTestCase> testCases)
        {
            _testCases = testCases;
        }

        public TestCollectionCategories Run(CancellationToken cancellationToken)
        {
            var result = new TestCollectionCategories();

            var collectionComparer = new TestCollectionComparer();
            var classComparer = new TestClassComparer();

            // group test cases by collections
            var testCollectionGroups = _testCases.GroupBy(testCase => testCase.TestMethod.TestClass.TestCollection, collectionComparer);
            foreach (var testCasesGroupedByCollection in testCollectionGroups)
            {
                // group collections by classes because a collection can be shared by multiple classes
                var testClassGroups = testCasesGroupedByCollection.GroupBy(tc => tc.TestMethod.TestClass, classComparer);
                foreach (var testCasesGroupedByClass in testClassGroups)
                {
                    // sort the test cases by class and [Isolated]
                    var sortedTestCases = SortTestCases(testCasesGroupedByClass, cancellationToken);

                    var collection = testCasesGroupedByCollection.Key;
                    result.AddTypemockPragmaticTestCases(sortedTestCases.TypemockPragmatic, collection);
                    result.AddTypemockInterfaceOnlyTestCases(sortedTestCases.TypemockInterfaceOnly, collection);
                    result.AddOtherTestCases(sortedTestCases.Other, collection);

                    if (cancellationToken.IsCancellationRequested) break;
                }

                if (cancellationToken.IsCancellationRequested) break;
            }

            return result;
        }

        private SortedTestCases SortTestCases(IGrouping<ITestClass, IXunitTestCase> testCasesGroupedByClass, CancellationToken cancellationToken)
        {
            var result = new SortedTestCases();

            // assign test cases to one of the result's lists based on [Isolated(DesignMode=...)]

            var testCaseCount = 0;
            // check if the test class is decorated with [Isolated] and get the DesignMode
            var classDesignMode = GetClassIsolatedDesignMode(testCasesGroupedByClass.Key.Class);
            foreach (var testCase in testCasesGroupedByClass)
            {
                // check if the method is decorated with [Isolated] and get the DesignMode
                var methodDesignMode = GetMethodIsolatedDesignMode(testCase.Method);

                switch (methodDesignMode)
                {
                    case TypemockDesignMode.Pragmatic:
                        result.TypemockPragmatic.Add(testCase);
                        break;
                    case TypemockDesignMode.InterfaceOnly:
                        result.TypemockInterfaceOnly.Add(testCase);
                        break;
                    case null:
                        // method is not decorated, check class
                        switch (classDesignMode)
                        {
                            case TypemockDesignMode.Pragmatic:
                                result.TypemockPragmatic.Add(testCase);
                                break;
                            case TypemockDesignMode.InterfaceOnly:
                                result.TypemockInterfaceOnly.Add(testCase);
                                break;
                            case null:
                                result.Other.Add(testCase);
                                break;
                        }
                        break;
                }
                testCaseCount++;
                if (cancellationToken.IsCancellationRequested) break;
            }

            if (result.Count != testCaseCount)
            {
                throw new InvalidOperationException($"Not all test cases in class \"{testCasesGroupedByClass.Key.Class.Name}\" were sorted");
            }
            return result;
        }

        private static TypemockDesignMode? GetClassIsolatedDesignMode(ITypeInfo testClassInfo)
        {
            var isolatedAttribute = testClassInfo.GetCustomAttributes(TypemockHelper.IsolatedAttributeType).SingleOrDefault();
            return isolatedAttribute?.GetNamedArgument<TypemockDesignMode>("Design");
        }

        private static TypemockDesignMode? GetMethodIsolatedDesignMode(IMethodInfo testMethodInfo)
        {
            var isolatedAttribute = testMethodInfo.GetCustomAttributes(TypemockHelper.IsolatedAttributeType).SingleOrDefault();
            return isolatedAttribute?.GetNamedArgument<TypemockDesignMode>("Design");
        }

        class SortedTestCases
        {
            public List<IXunitTestCase> TypemockPragmatic { get; } = new List<IXunitTestCase>();
            public List<IXunitTestCase> TypemockInterfaceOnly { get; } = new List<IXunitTestCase>();
            public List<IXunitTestCase> Other { get; } = new List<IXunitTestCase>();

            public int Count => TypemockPragmatic.Count + TypemockInterfaceOnly.Count + Other.Count;
        }
    }
}