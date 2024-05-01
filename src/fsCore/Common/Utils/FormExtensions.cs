using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
namespace Common.Utils
{
    public static class FormExtensions
    {
        public static async Task<byte[]> CompressImageIntoByteArray(this IFormFile file, int width = 720, int height = 576, int? quality = null, int maxSize = 1)
        {
            await using var stream = file.OpenReadStream();
            using var image = await Image.LoadAsync(stream);
            if (image.Width != width || image.Height != height)
            {
                image.Mutate(x => x.Resize(width, height));
            }
            if (((double)file.Length / 1000000) > maxSize)
            {
                await image.SaveAsync(stream, new JpegEncoder { Quality = quality ?? 50 });
            }
            return Convert.FromBase64String(image.ToBase64String(JpegFormat.Instance).TrimBase64String());
        }
        public static async Task<byte[]> ToByteArrayAsync(this IFormFile file)
        {
            await using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
    }
}