﻿using System;
using Autofac;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.DatabaseTests {

	public class DatabaseTestContext<TTarget> {

		private IContainer Container {
			get { return TestContainerManager.Container; }
		}

		public TResult RunTest<TResult>(Func<TTarget, TResult> func) {

			using (Container.BeginLifetimeScope()) {
				
				var target = Container.Resolve<TTarget>();
				return func(target);

			}

		}

	}

	public static class TestContainerManager {

		private static void EnsureContainerInitialized() {

			lock (containerLock) {
				if (container == null) {
					container = TestContainerFactory.BuildContainer();					
					testDatabase = container.Resolve<TestDatabase>();
				}
			}

		}

		private static IContainer container;
		private const string containerLock = "container";
		private static TestDatabase testDatabase;

		public static IContainer Container {
			get {
				EnsureContainerInitialized();
				return container;
			}
		}

		public static TestDatabase TestDatabase {
			get {
				EnsureContainerInitialized();
				return testDatabase;
			}
		}

	}

}
