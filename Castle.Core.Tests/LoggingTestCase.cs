// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.DynamicProxy.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using Castle.Core.Logging;
	using NUnit.Framework;

	[TestFixture]
	public class LoggingTestCase
	{
		[Test]
		public void CacheMiss()
		{
			// Arrange
			CollectingLogger logger = new CollectingLogger();
			ProxyGenerator generator = new ProxyGenerator { Logger = logger };

			// Act
			generator.CreateClassProxy<EmptyClass>();

			// Assert
			Assert.That(logger.RecordedMessage(LoggerLevel.Debug, "No cached proxy type was found for target type " +
				"Castle.DynamicProxy.Tests.LoggingTestCase+EmptyClass."));
		}

		[Test]
		public void CacheHitClassProxy()
		{
			// Arrange
			CollectingLogger logger = new CollectingLogger();
			ProxyGenerator generator = new ProxyGenerator { Logger = logger };

			// Act
			generator.CreateClassProxy<EmptyClass>();
			generator.CreateClassProxy<EmptyClass>();

			// Assert
			Assert.That(logger.RecordedMessage(LoggerLevel.Debug, "Found cached proxy type Castle.Proxies.EmptyClassProxy " +
				"for target type Castle.DynamicProxy.Tests.LoggingTestCase+EmptyClass."));
		}

		[Test]
		public void CacheHitInterfaceProxy()
		{
			// Arrange
			CollectingLogger logger = new CollectingLogger();
			ProxyGenerator generator = new ProxyGenerator { Logger = logger };

			// Act
			generator.CreateInterfaceProxyWithoutTarget<IEmptyInterface>();
			generator.CreateInterfaceProxyWithoutTarget<IEmptyInterface>();

			// Assert
			Assert.That(logger.RecordedMessage(LoggerLevel.Debug, "Found cached proxy type Castle.Proxies.IEmptyInterfaceProxy " +
				"for target type Castle.DynamicProxy.Tests.LoggingTestCase+IEmptyInterface."));
		}

		[Test]
		public void ProxyGenerationOptionsEqualsAndGetHashCodeNotOverriden()
		{
			// Arrange
			CollectingLogger logger = new CollectingLogger();
			ProxyGenerator generator = new ProxyGenerator { Logger = logger };

			// Act
			ProxyGenerationOptions options = new ProxyGenerationOptions {
				Hook = new EmptyHook()
			};
			generator.CreateClassProxy(typeof(EmptyClass), options);

			// Assert
			Assert.That(logger.RecordedMessage(LoggerLevel.Warn, "The IProxyGenerationHook type " +
				"Castle.DynamicProxy.Tests.LoggingTestCase+EmptyHook does not override both Equals and GetHashCode. " +
				"If these are not correctly overridden caching will fail to work causing performance problems."));
		}

#if !SILVERLIGHT
		[Test]
		public void ExcludedNonVirtualMethods()
		{
			// Arrange
			CollectingLogger logger = new CollectingLogger();
			ProxyGenerator generator = new ProxyGenerator { Logger = logger };

			// Act
			generator.CreateClassProxy<NonVirtualMethodClass>();

			// Assert
			Assert.That(logger.RecordedMessage(LoggerLevel.Debug, "Excluded non-virtual method ClassMethod on " +
				"Castle.DynamicProxy.Tests.LoggingTestCase+NonVirtualMethodClass because it cannot be intercepted."));
			Assert.That(logger.RecordedMessage(LoggerLevel.Debug, "Excluded sealed method InterfaceMethod on " +
				"Castle.DynamicProxy.Tests.LoggingTestCase+NonVirtualMethodClass because it cannot be intercepted."));
		}
#endif

		[Test]
		public void FoundExplicitlyImplementedInterfaceMethods()
		{
			// Arrange
			CollectingLogger logger = new CollectingLogger();
			ProxyGenerator generator = new ProxyGenerator { Logger = logger };

			// Act
			generator.CreateClassProxy<ClassWithInterfaceMethodExplicitlyImplemented>();

			// Assert
			Assert.That(logger.RecordedMessage(LoggerLevel.Debug, "Excluded explicitly implemented interface method " +
				"Castle.DynamicProxy.Tests.LoggingTestCase.ISingleMethodInterface.InterfaceMethod on type " +
				"Castle.DynamicProxy.Tests.LoggingTestCase+ClassWithInterfaceMethodExplicitlyImplemented " +
				"because it cannot be intercepted."));
		}
		#region Test Types

		public class EmptyClass
		{
		}

		public interface IEmptyInterface
		{
		}

		public interface ISingleMethodInterface
		{
			void InterfaceMethod();
		}

		public class NonVirtualMethodClass : ISingleMethodInterface
		{
			public void ClassMethod()
			{
			}

			public void InterfaceMethod()
			{
			}
		}

		public class ClassWithInterfaceMethodExplicitlyImplemented : ISingleMethodInterface
		{
			void ISingleMethodInterface.InterfaceMethod()
			{
			}
		}

		public class EmptyHook : IProxyGenerationHook
		{
			public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
			{
				return true;
			}

			public void NonProxyableMemberNotification(Type type, MemberInfo memberInfo)
			{
			}

			public void MethodsInspected()
			{
			}
		}

		#endregion
	}

	#region CollectingLogger

	public class CollectingLogger : ILogger
	{
		private readonly List<string> messages = new List<string>();

		public bool RecordedMessage(LoggerLevel level, string message)
		{
			return messages.Contains(level.ToString().ToUpper() + ": " + message);
		}

		public void Debug(string message)
		{
			throw new NotImplementedException();
		}

		public void Debug(string message, Exception exception)
		{
			throw new NotImplementedException();
		}

		public void Debug(string format, params object[] args)
		{
			messages.Add("DEBUG: " + string.Format(format, args));
		}

		public void DebugFormat(string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void DebugFormat(Exception exception, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void DebugFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void Info(string message)
		{
			throw new NotImplementedException();
		}

		public void Info(string message, Exception exception)
		{
			throw new NotImplementedException();
		}

		public void Info(string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void InfoFormat(string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void InfoFormat(Exception exception, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void InfoFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void Warn(string message)
		{
			throw new NotImplementedException();
		}

		public void Warn(string message, Exception exception)
		{
			throw new NotImplementedException();
		}

		public void Warn(string format, params object[] args)
		{
			messages.Add("WARN: " + string.Format(format, args));
		}

		public void WarnFormat(string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void WarnFormat(Exception exception, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void WarnFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void Error(string message)
		{
			throw new NotImplementedException();
		}

		public void Error(string message, Exception exception)
		{
			throw new NotImplementedException();
		}

		public void Error(string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void ErrorFormat(string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void ErrorFormat(Exception exception, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void ErrorFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void Fatal(string message)
		{
			throw new NotImplementedException();
		}

		public void Fatal(string message, Exception exception)
		{
			throw new NotImplementedException();
		}

		public void Fatal(string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void FatalFormat(string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void FatalFormat(Exception exception, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void FatalFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void FatalError(string message)
		{
			throw new NotImplementedException();
		}

		public void FatalError(string message, Exception exception)
		{
			throw new NotImplementedException();
		}

		public void FatalError(string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public ILogger CreateChildLogger(string loggerName)
		{
			throw new NotImplementedException();
		}

		public bool IsDebugEnabled
		{
			get { return true; }
		}

		public bool IsInfoEnabled
		{
			get { return true; }
		}

		public bool IsWarnEnabled
		{
			get { return true; }
		}

		public bool IsErrorEnabled
		{
			get { return true; }
		}

		public bool IsFatalEnabled
		{
			get { return true; }
		}

		public bool IsFatalErrorEnabled
		{
			get { return true; }
		}
	}

	#endregion
}