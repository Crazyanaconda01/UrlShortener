using System;
using Xunit;
using UrlShortener.Data;
using Moq;
using System.Collections.Generic;
using AutoFixture;
using System.Linq;
using System.Threading.Tasks;
using UrlShortener.General;

namespace UrlShortener.Tests
{
    public class Tests
    {
        [Fact]
        public void Test_Add_Url()
        {
            //Arrange
            TheUrl url = new TheUrl() { Counter = 0, CreatedDate = DateTime.Now, IP = "192.168.1.1", OriginalUrl = "http://google.com", ShortUrl = "9999999999" };
            var itemInserted = new List<TheUrl>();
            Mock<IUrlRepo> _mock = new Mock<IUrlRepo>();

            //Act
            _mock.Setup(d => d.AddUrl(It.IsAny<TheUrl>())).Callback<TheUrl>((s) => itemInserted.Add(s));
            var result = _mock.Object.GetUrlByShortUrl(url.ShortUrl);

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_Get_Url_By_Shortcode()
        {
            //Arrange
            Mock<IUrlRepo> _mock = new Mock<IUrlRepo>();
            List<TheUrl> urlList = new List<TheUrl>();

            urlList.Add(new TheUrl { Counter = 0, CreatedDate = DateTime.Now, IP = "192.168.1.1", OriginalUrl = "http://google.com", ShortUrl = "9999999997" });
            urlList.Add(new TheUrl { Counter = 0, CreatedDate = DateTime.Now, IP = "192.168.1.2", OriginalUrl = "http://google.com", ShortUrl = "9999999998" });
            urlList.Add(new TheUrl { Counter = 0, CreatedDate = DateTime.Now, IP = "192.168.1.3", OriginalUrl = "http://google.com", ShortUrl = "9999999999" });

            foreach(var item in urlList)
            {
                _mock.Setup(d => d.AddUrl(item));
            }

            //Act
            var result = _mock.Object.GetUrlByShortUrl(urlList.ElementAt(2).ShortUrl);

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_Get_Single_Url()
        {
            //Arrange
            Mock<IUrlRepo> _mock = new Mock<IUrlRepo>();
            List<TheUrl> urlList = new List<TheUrl>();

            urlList.Add(new TheUrl { Id = "asdf8fasg6", Counter = 0, CreatedDate = DateTime.Now, IP = "192.168.1.1", OriginalUrl = "http://google.com", ShortUrl = "9999999997" });

            foreach (var item in urlList)
            {
                _mock.Setup(d => d.AddUrl(item));
            }

            //Act
            var result = _mock.Object.GetUrl(urlList.ElementAt(0).ShortUrl);

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_Url_Is_InValid_No_Colon()
        {
            //Arrange
            var url = "http//www.google.com";

            //Act
            var result = Functions.CheckValidUrl(url);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void Test_Url_Is_Valid()
        {
            //Arrange
            var url = "http://www.google.com";

            //Act
            var result = Functions.CheckValidUrl(url);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void Test_Url_Is_Valid_No_HTTP()
        {
            //Arrange
            var url = "www.google.com";

            //Act
            var result = Functions.CheckValidUrl(url);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void Test_Url_Is_Valid_No_WWW()
        {
            //Arrange
            var url = "google.com";

            //Act
            var result = Functions.CheckValidUrl(url);

            //Assert
            Assert.True(result);
        }

    }
}
