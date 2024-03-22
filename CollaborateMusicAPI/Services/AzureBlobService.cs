using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace ALIVEMusicAPI.Services;

public interface IAzureBlobService
{
    Task<string> UploadFileAsync(string blobContainerName, string blobName, Stream content);
    Task<string> GetBlobSasUrl(string blobContainerName, string blobName);
    Task ReplaceBlobAsync(string blobContainerName, string blobName, Stream newContent);
    string AccountName { get; }
}

public class AzureBlobService : IAzureBlobService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly StorageSharedKeyCredential _storageSharedKeyCredential;
    public string AccountName { get; private set; }

    public AzureBlobService(string connectionString)
    {
        var storageAccountName = connectionString.Split(new[] { ";AccountName=" }, StringSplitOptions.None)[1]
            .Split(';')[0];
        var storageAccountKey = connectionString.Split(new[] { ";AccountKey=" }, StringSplitOptions.None)[1]
            .Split(';')[0];

        _storageSharedKeyCredential = new StorageSharedKeyCredential(storageAccountName, storageAccountKey);
        _blobServiceClient = new BlobServiceClient(connectionString);
        AccountName = storageAccountName;
    }

    public async Task<string> UploadFileAsync(string blobContainerName, string blobName, Stream content)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(blobContainerName);
        await blobContainerClient.CreateIfNotExistsAsync();

        var blobClient = blobContainerClient.GetBlobClient(blobName);
        await blobClient.UploadAsync(content, overwrite: true);

        return blobClient.Uri.ToString();
    }

    public async Task ReplaceBlobAsync(string blobContainerName, string blobName, Stream newContent)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(blobContainerName);
        var blobClient = blobContainerClient.GetBlobClient(blobName);

        // Delete the existing blob
        if (await blobClient.ExistsAsync())
        {
            await blobClient.DeleteIfExistsAsync();
        }

        // Upload the new file
        await blobClient.UploadAsync(newContent, overwrite: true);
    }

    public async Task<string> GetBlobSasUrl(string blobContainerName, string blobName)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(blobContainerName);
        var blobClient = blobContainerClient.GetBlobClient(blobName);

        // Check if the blob exists
        if (!(await blobClient.ExistsAsync()))
        {
            // Handle the case where the blob does not exist
            // You might want to return null or throw an exception
            return null!;
        }

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = blobContainerClient.Name,
            BlobName = blobClient.Name,
            Resource = "b", // b for blob
            StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5), // Start 5 minutes in the past
            ExpiresOn = DateTimeOffset.UtcNow.AddHours(1),
        };

        // Allow read permissions
        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        var sasToken = sasBuilder.ToSasQueryParameters(_storageSharedKeyCredential).ToString();
        await blobClient.GetPropertiesAsync();
        // Return the full SAS URL
        return $"{blobClient.Uri}?{sasToken}";
    }


}
