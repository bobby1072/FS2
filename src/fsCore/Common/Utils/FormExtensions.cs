using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
namespace Common.Utils
{
    public static class FormExtensions
    {
        public static async Task<byte[]> ToByteArrayAsync(this IFormFile file, int? width = null, int? height = null, double? maxSize = null)
        {
            if (width is null && height is null && maxSize is null)
            {
                await using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
            await using var stream = file.OpenReadStream();
            using var image = await Image.LoadAsync(stream);
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
            if (((double)file.Length / 1000000) > maxSize)
            {
                await image.SaveAsync(stream, new JpegEncoder { Quality = 50 });
            }
            return Convert.FromBase64String(image.ToBase64String(JpegFormat.Instance).TrimBase64String());
        }
    }
}