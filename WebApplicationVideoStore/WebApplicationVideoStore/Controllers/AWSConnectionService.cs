using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplicationVideoStore.Models;

namespace WebApplicationVideoStore.Controllers
{
    public class AWSConnectionService
    {
        public static AmazonDynamoDBClient client = null;
        public readonly static string registrationTableName = "RegistrationTable";
        public readonly static string fileTableName = "lab3videos";
        public const string s3StorageBucketName = "lab3videobucket";
        public readonly RegionEndpoint s3StorageBucketRegion = RegionEndpoint.USEast1;
        public DynamoDBContext context { get; }
        public BasicAWSCredentials credentials { get; }
        public IAmazonS3 s3Client;
        private static AWSConnectionService singletonInstance;
        private AWSConnectionService()
        {
            credentials = new BasicAWSCredentials("AKIASKUR6LMBRQ4LIMN7", "u8hTHiV4GxqFC1kLyhsdvO6wchyDnm/mx6Gr7xnC");
            client = new AmazonDynamoDBClient(credentials, RegionEndpoint.USEast1);
            context = new DynamoDBContext(client);
            s3Client = new AmazonS3Client(credentials, s3StorageBucketRegion);
        }

        public static AWSConnectionService getInstance()
        {
            if (singletonInstance != null)
                return singletonInstance;
            else
            {
                singletonInstance = new AWSConnectionService();
                return singletonInstance;
            }
        }

        public async Task uploadFileAndUpdateTable(Stream x, string filename, string ownerEmailId)
        {
            try
            {
                VideoLibrary video = new VideoLibrary(ownerEmailId, filename);

                var fileTransferUtility = new TransferUtility(s3Client);

                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = x,
                    BucketName = s3StorageBucketName,
                    Key = video.GeneratedFileNameAsMediaId,
                    CannedACL = S3CannedACL.PublicRead
                };

                await Task.Run(async () =>
                {
                    Task upld = fileTransferUtility.UploadAsync(uploadRequest);
                    while (!upld.IsCompleted) {
                        upld.Wait(25);
                        if (upld.IsFaulted || upld.IsCanceled)
                            break;
                    }
                    if(upld.IsFaulted) { Trace.WriteLine(upld.Exception); throw upld.Exception; }
                    else if (upld.IsCompletedSuccessfully)
                    {
                        Trace.WriteLine(String.Format("Upload status: {0} \n Upload Complete: {1}", upld.Status, upld.IsCompleted));
                        Trace.WriteLine("File Upload completed.. Check bucket via console");
                        await new DynamoDBContext(client).SaveAsync(video);
                        return;
                    }
                    
                });
            }
            catch (AmazonDynamoDBException ex) { Trace.WriteLine(ex.Message); }
            catch (AmazonS3Exception ex) { Trace.WriteLine(ex.Message); }
            catch (Exception ex) { Trace.WriteLine(ex.Message); }
        }
    }
}
