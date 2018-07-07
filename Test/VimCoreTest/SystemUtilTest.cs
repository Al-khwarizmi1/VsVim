﻿using System;
using Xunit;

namespace Vim.UnitTest
{
    public sealed class SystemUtilTest : IDisposable
    {
        private readonly string _savedHome;
        private readonly string _savedHomeDrive;
        private readonly string _savedHomePath;

        public SystemUtilTest()
        {
            _savedHome = Environment.GetEnvironmentVariable("HOME");
            _savedHomeDrive = Environment.GetEnvironmentVariable("HOMEDRIVE");
            _savedHomePath = Environment.GetEnvironmentVariable("HOMEPATH");
        }

        public void Dispose()
        {
            Environment.SetEnvironmentVariable("HOME", _savedHome);
            Environment.SetEnvironmentVariable("HOMEDRIVE", _savedHomeDrive);
            Environment.SetEnvironmentVariable("HOMEPATH", _savedHomePath);
        }

        /// <summary>
        /// %HOME% should win here
        /// </summary>
        [Fact]
        public void GetHome_PreferHome()
        {
            Environment.SetEnvironmentVariable("HOME", @"c:\foo");
            Assert.Equal(@"c:\foo", SystemUtil.GetHome());
        }

        /// <summary>
        /// If %HOME% is not present then it should prefer %HOMEDRIVE%%HOMEPATH%
        /// </summary>
        [Fact]
        public void GetHome_ThenPreferHomePathAndDrive()
        {
            Environment.SetEnvironmentVariable("HOME", null);
            Environment.SetEnvironmentVariable("HOMEDRIVE", @"e:\");
            Environment.SetEnvironmentVariable("HOMEPATH", @"bar");
            Assert.Equal(@"e:\bar", SystemUtil.GetHome());
        }

        /// <summary>
        /// Don't resolve paths that don't start with a ~
        /// </summary>
        [Fact]
        public void ResolvePath_None()
        {
            Assert.Equal(@"c:\foo", SystemUtil.ResolvePath(@"c:\foo"));
        }

        [Fact]
        public void ResolvePath_Simple()
        {
            Environment.SetEnvironmentVariable("HOME", @"c:\foo");
            Assert.Equal(@"c:\foo\bar", SystemUtil.ResolvePath(@"~\bar"));
        }

        [Fact]
        public void ResolvePath_Lowercase()
        {
            Environment.SetEnvironmentVariable("lowercase", @"c:\foo");
            Assert.Equal(@"c:\foo\bar", SystemUtil.ResolvePath(@"$lowercase\bar"));
        }

        [Fact]
        public void ResolvePath_Underscore()
        {
            Environment.SetEnvironmentVariable("var_with_underscore", @"c:\foo");
            Assert.Equal(@"c:\foo\bar", SystemUtil.ResolvePath(@"$var_with_underscore\bar"));
        }

        /// <summary>
        /// Test the cases we expect to work for CombinePath
        /// </summary>
        [Fact]
        public void CombinePath()
        {
            Assert.Equal(@"c:\foo", SystemUtil.CombinePath(@"c:", @"\foo"));
            Assert.Equal(@"c:\foo", SystemUtil.CombinePath(@"c:\", @"\foo"));
            Assert.Equal(@"c:\foo", SystemUtil.CombinePath(@"c:\", @"foo"));
        }

        [Fact]
        public void ResolveVimPath_Directory()
        {
            Assert.Equal(@"c:\foo", SystemUtil.ResolveVimPath(@"c:\foo", "."));
            Assert.Equal(@"c:\", SystemUtil.ResolveVimPath(@"c:\foo", ".."));
        }

        [Fact]
        public void StripPathPrefix()
        {
            Assert.Equal(@"C\foo.bar", SystemUtil.StripPathPrefix(@"C:\A\B\", "C:/A/B/C/foo.bar"));
            Assert.Equal(@"C:\A\B\C1\D", SystemUtil.StripPathPrefix(@"C:\A\B\C\D", @"C:\A\B\C1\D"));
            Assert.Equal(@"C:\A\B\C1\D\foo.bar", SystemUtil.StripPathPrefix(@"C:\A\B\C\D\", "C:/A/B/C1/D/foo.bar"));
            Assert.Equal(@"C:\A\B\C\D", SystemUtil.StripPathPrefix(@"D:\A\B\C\D", "C:/A/B/C/D"));
            Assert.Equal(@"C:\A\B\C\foo.bar", SystemUtil.StripPathPrefix("", "C:/A/B/C/foo.bar"));
            Assert.Equal("", SystemUtil.StripPathPrefix("", ""));
            Assert.Equal("foo.baz", SystemUtil.StripPathPrefix("foo.bar", "foo.baz"));
        }
    }
}
