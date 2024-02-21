using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using Amazon.S3.Model;

namespace cctViewer;

public class S3Service
{
    // TODO: SECURE THE KEYS
    // KEYS stored in enviroment variables
    // private readonly string _AWSAccessKey = ""; // GIVEN
    // private readonly string _AWSSecretKey = ""; // GIVEN
    // private readonly string _bucketName = ""; // GIVEN
    private readonly string _userName = "";
    private IAmazonS3 client;
    
    public S3Service()
    {
        AWSCredentials credentials = new EnvironmentVariablesAWSCredentials();
        client = new AmazonS3Client(credentials);
    }

    public async Task<List<Video>> GetVideos()
    {
        var request = new ListObjectsV2Request
        {
            BucketName = "",
            ExpectedBucketOwner = "",
            Prefix = $"/{_userName}"
        };
        
        List<Video> videos = new();

        ListObjectsV2Response response;
        List<S3Object> objects;
        do
        {
            response = await client.ListObjectsV2Async(request);
            objects = response.S3Objects;

            // If the response is truncated, set the request ContinuationToken
            // from the NextContinuationToken property of the response.
            request.ContinuationToken = response.NextContinuationToken;
        } while (response.IsTruncated);
        
        // After getting all the videos, process them into Video list
        foreach (var obj in objects)
        {
            var video = new Video
            {
                Title = obj.Key,
                Url = obj.Key // TODO: Get URL??
            };
            videos.Add(video);
        }

        return videos;
    }
}