// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using ALCProxy.Proxy;
using Xunit;

namespace ALCProxy.Tests
{
    public interface ITest
    {
        public string DoThing();

        public int DoThing2(int a, List<string> list);
    }
    public class Test2 { }
    public class Test : ITest
    {
        public string DoThing()
        {
            var a = Assembly.GetExecutingAssembly();
            return AssemblyLoadContext.GetLoadContext(a).Name;
        }
        public int DoThing2(int a, List<string> list)
        {
            Console.WriteLine(a);

            return a + list[0].Length;
        }
    }
    public class ALCProxyTests
    {
        [Fact]
        public void TestBasicContextLoading()
        {
            System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);
            AssemblyLoadContext alc = new AssemblyLoadContext("TestContext", isCollectible: true);
            ITest t = ProxyBuilder<ITest>.CreateInstanceAndUnwrap(alc, Assembly.GetExecutingAssembly().CodeBase.Substring(8), "Test");
            Assert.Equal("TestContext", t.DoThing());
            Assert.Equal(17, t.DoThing2(10, new List<string> { "Letters", "test", "hello world!"}));
        }
    }
}