namespace MusicGreeter.Common;
public static class StorageConnector
{
    public static async Task<bool> AddUserToStorageAsync(Employee user, ConfigValues values)
    {
        if (user.Image == null || user.Id.Equals(Guid.Empty))
        {
            return false;
        }
        var credentials = new StorageCredentials(values.StorageAccountName, values.StorageAccountKey);
        var blob = new CloudBlockBlob(new Uri($"{values.StorageAccountUrl}{user.Id}"), credentials);
        blob.Metadata.Add("FirstName", HttpUtility.UrlEncode(user.FirstName));
        blob.Metadata.Add("LastName", HttpUtility.UrlEncode(user.LastName));
        blob.Metadata.Add("Id", HttpUtility.UrlEncode(user.Id.ToString()));
        blob.Metadata.Add("SpotifyUrl", HttpUtility.UrlEncode(user.SpotifyUrl));
        await blob.UploadFromByteArrayAsync(user.Image, 0, user.Image.Length);
        return true;
    }

    public static IEnumerable<Employee> GetEmployees(ConfigValues values)
    {
        var storageAccount = CloudStorageAccount.Parse(values.StorageConnectionString);
        var blobClient = storageAccount.CreateCloudBlobClient();
        var container = blobClient.GetContainerReference(values.ContainerName);
        foreach (var c in container.ListBlobs(null, false, BlobListingDetails.Metadata))
        {
            if (c.GetType() == typeof(CloudBlockBlob))
            {
                var blob = (CloudBlockBlob)c;
                yield return new Employee
                {
                    FirstName = HttpUtility.UrlDecode(blob.Metadata["FirstName"]),
                    LastName = HttpUtility.UrlDecode(blob.Metadata["FirstName"]),
                    Id = Guid.Parse(HttpUtility.UrlDecode(blob.Metadata["Id"])),
                };
            }
        }
    }
}

