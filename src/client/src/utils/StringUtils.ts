export const base64StringToJpegFile = (base64String: string): File => {
  const byteCharacters = atob(base64String);
  const byteNumbers = new Array(byteCharacters.length);
  for (let i = 0; i < byteCharacters.length; i++) {
    byteNumbers[i] = byteCharacters.charCodeAt(i);
  }
  const byteArray = new Uint8Array(byteNumbers);

  const blob = new Blob([byteArray], { type: "image/jpeg" });

  const file = new File([blob], "image.jpg", { type: "image/jpeg" });
  return file;
};
