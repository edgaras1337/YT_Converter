Feature: Convert video to mp3 and mp4 formats

Video downloads ftom the specified link, and it is then
converted to MP3 and MP4 formats and saved.
Saved files are ready to be downloaded at any time.

Scenario: Video converted successfully
	Given URL of the video to convert "https://www.youtube.com/watch?v=tvTRZJ-4EyI&ab_channel=KendrickLamarVEVO"
	When Video URL is passed
	Then Video gets converted successfully

Scenario: Audio downloaded successfully
	Given URL of the video to convert and download audio "https://www.youtube.com/watch?v=tvTRZJ-4EyI&ab_channel=KendrickLamarVEVO"
	When Audio file name is passed
	Then Audio is availible to download

Scenario: Video downloaded successfully
	Given URL of the video to convert and download video "https://www.youtube.com/watch?v=tvTRZJ-4EyI&ab_channel=KendrickLamarVEVO"
	When Video file name is passed
	Then Video is availible to download