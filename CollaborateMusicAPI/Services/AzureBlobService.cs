using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace ALIVEMusicAPI.Services;

public interface IAzureBlobService
{
    Task<string> UploadFileAsync(string blobContainerName, string blobName, Stream content);
    string GetBlobSasUrl(string blobContainerName, string blobName);
}

public class AzureBlobService : IAzureBlobService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly StorageSharedKeyCredential _storageSharedKeyCredential;

    public AzureBlobService(string connectionString)
    {
        var storageAccountName = connectionString.Split(new[] { ";AccountName=" }, StringSplitOptions.None)[1]
            .Split(';')[0];
        var storageAccountKey = connectionString.Split(new[] { ";AccountKey=" }, StringSplitOptions.None)[1]
            .Split(';')[0];

        _storageSharedKeyCredential = new StorageSharedKeyCredential(storageAccountName, storageAccountKey);
        _blobServiceClient = new BlobServiceClient(connectionString);
    }

    public async Task<string> UploadFileAsync(string blobContainerName, string blobName, Stream content)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(blobContainerName);
        await blobContainerClient.CreateIfNotExistsAsync();

        var blobClient = blobContainerClient.GetBlobClient(blobName);
        await blobClient.UploadAsync(content, overwrite: true);

        return blobClient.Uri.ToString();
    }

    public string GetBlobSasUrl(string blobContainerName, string blobName)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(blobContainerName);
        var blobClient = blobContainerClient.GetBlobClient(blobName);

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = blobContainerClient.Name,
            BlobName = blobClient.Name,
            Resource = "b", // b for blob
            StartsOn = DateTimeOffset.UtcNow,
            ExpiresOn = DateTimeOffset.UtcNow.AddDays(7), // SAS URL will be valid for 1 hour
        };

        // Allow read permissions
        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        var sasToken = sasBuilder.ToSasQueryParameters(_storageSharedKeyCredential).ToString();

        // Return the full SAS URL
        return $"{blobClient.Uri}?{sasToken}";
    }

}
