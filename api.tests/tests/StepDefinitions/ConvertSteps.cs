using Newtonsoft.Json;
using SpecFlow.Internal.Json;
using System;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Web;
using tests2.DTOs;

namespace tests2.StepDefinitions
{
    [Binding]
    public class ConvertVideoToMp3AndMp4FormatsStepDefinitions
    {
        private readonly ScenarioContext _scenarioContext;

        public ConvertVideoToMp3AndMp4FormatsStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        private async Task<HttpResponseMessage> ConvertVideo(string url)
        {
            var _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:8001") };

            return await _httpClient.PostAsync($"/api/convert?url={HttpUtility.UrlEncode(url)}", null);
        }

        [Given(@"URL of the video to convert ""(.*)""")]
        public void GivenURL(string url)
        {
            _scenarioContext["URL"] = url;
        }

        [When(@"Video URL is passed")]
        public async Task WhenVideoURLIsPassed()
        {
            var response = await ConvertVideo(_scenarioContext["URL"].ToString()!);

            _scenarioContext.Add("ConvertStatus", response.StatusCode);
        }

        [Then(@"Video gets converted successfully")]
        public void ThenVideoGetsConvertedSuccessfully()
        {
            _scenarioContext["ConvertStatus"].Should().Be(HttpStatusCode.OK);
        }


        [Given(@"URL of the video to convert and download audio ""(.*)""")]
        public async Task GivenURLForAudioDownload(string url)
        {
            var response = await ConvertVideo(url);

            var content = await response.Content.ReadFromJsonAsync<ResponseDTO>();
            _scenarioContext.Add("AudioURL", content?.AudioURL);
        }

        [When(@"Audio file name is passed")]
        public async Task WhenAudioFileNameIsPassed()
        { 
            var _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:8001") };

            var response = await _httpClient.GetAsync(_scenarioContext["AudioURL"].ToString());

            _scenarioContext.Add("AudioDownloadStatus", response.StatusCode);
        }

        [Then(@"Audio is availible to download")]
        public void ThenAudioIsAvailableToDownload()
        {
            _scenarioContext["AudioDownloadStatus"].Should().Be(HttpStatusCode.OK);
        }


        [Given(@"URL of the video to convert and download video ""(.*)""")]
        public async Task GivenVideoFileName(string url)
        {
            var response = await ConvertVideo(url);

            var content = await response.Content.ReadFromJsonAsync<ResponseDTO>();
            _scenarioContext.Add("VideoURL", content?.VideoURL);
        }

        [When(@"Video file name is passed")]
        public async Task WhenVideoFileNameIsPassed()
        {
            var _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:8001") };

            var response = await _httpClient.GetAsync(_scenarioContext["VideoURL"].ToString());

            _scenarioContext.Add("VideoDownloadStatus", response.StatusCode);
        }

        [Then(@"Video is availible to download")]
        public void ThenVideoIsAvailableToDownload()
        {
            _scenarioContext["VideoDownloadStatus"].Should().Be(HttpStatusCode.OK);
        }
    }
}
