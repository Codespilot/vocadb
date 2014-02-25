using System;
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
		
		private static IContainer container;
		private const string containerLock = "container";
		public static readonly TestDatabase TestDatabase = new TestDatabase();

		public static IContainer Container {
			get {
				lock (containerLock) {
					return container ?? (container = TestContainerFactory.BuildContainer(TestDatabase));
				}
			}
		}

	}

}
