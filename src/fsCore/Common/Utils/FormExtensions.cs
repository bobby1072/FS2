using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
namespace Common.Utils
{
    public static class FormExtensions
    {
        public static async Task<byte[]> ToByteArrayAsync(this IFormFile file, decimal maxSize = 5)
        {
            if (((decimal)file.Length / 1000000) > maxSize)
            {
                await using var compressStream = file.OpenReadStream();
                using var image = await Image.LoadAsync(compressStream);
                await image.SaveAsync(compressStream, new JpegEncoder { Quality = 50 });
                return Convert.FromBase64String(image.ToBase64String(JpegFormat.Instance).TrimBase64String());
            }
            await using var basicStream = new MemoryStream();
            await file.CopyToAsync(basicStream);
            return basicStream.ToArray();
        }

        public static async Task<byte[]> ToByteArrayAsync(this IFormFile file, int? width = null, int? height = null, decimal? maxSize = null)
        {
            if (width is null && height is null && maxSize is null)
            {
                await using var basicStream = new MemoryStream();
                await file.CopyToAsync(basicStream);
                return basicStream.ToArray();
            }
            await using var compressStream = file.OpenReadStream();
            using var image = await Image.LoadAsync(compressStream);
            if (width is int foundWidth && height is int foundHeight)
            {
                image.Mutate(x => x.Resize(foundWidth, foundHeight));
            }
            else if (width is not null)
            {
                image.Mutate(x => x.Resize(width.Value, image.Height));
            }
            else if (height is not null)
            {
                image.Mutate(x => x.Resize(image.Width, height.Value));
            }
            if (((decimal)file.Length / 1000000) > maxSize)
            {
                await image.SaveAsync(compressStream, new JpegEncoder { Quality = 50 });
            }
            return Convert.FromBase64String(image.ToBase64String(JpegFormat.Instance).TrimBase64String());
        }
    }
}