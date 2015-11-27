// aws s3에서 파일 다운로드
var awsAccessKey = "";
var awsPassword = "";
var bucketName = "";

void S3_FileDownload(string downFile, string targetFolderPath)
{
  logger.Info(string.Format("[S3_FileDownload] S3에서 다운로드 시도. 버킷:{0}, 다운로드 파일:{1}, 받을 위치:{2}", bucketName, downFile, targetFolderPath));

  try
  {
  	var s3Client = GetAmazonS3Client();
    if(s3Client == null)
    {
        return;
    }

  	var keyName = downFile;
  	var request = new Amazon.S3.Model.GetObjectRequest
  	{
  		BucketName = bucketName,
  		Key = keyName
  	};

  	using (var response = s3Client.GetObject(request))
  	{
  		string dest = Path.Combine(targetFolderPath, keyName);
  		response.WriteResponseStreamToFile(dest);

      logger.Info("[S3_FileDownload] S3에서 다운로드 " + dest);
    }
  }
  catch (Exception ex)
  {
  	logger.Error("[S3_FileDownload] " + ex.Message);
  }
}

Amazon.S3.IAmazonS3 GetAmazonS3Client()
{
  if(string.IsNullOrEmpty(awsAccessKey) || string.IsNullOrEmpty(awsPassword) ||
     string.IsNullOrEmpty(bucketName))
  {
    logger.Error("AWS 접근 키 혹은 버킷 이름이 설정 되지 않았습니다");
    return null;
  }

  var config = new Amazon.S3.AmazonS3Config();
  config.RegionEndpoint = Amazon.RegionEndpoint.APNortheast1;
  var s3Client = Amazon.AWSClientFactory.CreateAmazonS3Client(awsAccessKey, awsPassword, config);
  return s3Client;
}

void WriteLogS3Config()
{
  logger.Info(string.Format("AWS 접근키:{0}, 패스워드:{1}, 버킷:{2}", awsAccessKey, awsPassword, bucketName));
}
