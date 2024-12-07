using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace Osmismerky.OCR
{
    internal class OsmiOcrTool
    {
        // Replace with your own Azure Computer Vision subscription key and endpoint
        private static readonly string subscriptionKey = "YOUR_AZURE_COMPUTER_VISION_SUBSCRIPTION_KEY";
        private static readonly string endpoint = "YOUR_AZURE_COMPUTER_VISION_ENDPOINT";

        public static async Task<string> ExtractTextFromImageAsync(string imagePath)
        {
            // Create a client for Computer Vision API
            ComputerVisionClient client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(subscriptionKey))
            {
                Endpoint = endpoint
            };

            // Load the image file
            using (Stream imageStream = File.OpenRead(imagePath))
            {
                // Perform OCR on the image
                OcrResult ocrResult = await client.RecognizePrintedTextInStreamAsync(true, imageStream);

                // Extract the text from the OCR result
                return ParseOcrResult(ocrResult);
            }
        }

        private static string ParseOcrResult(OcrResult ocrResult)
        {
            string extractedText = string.Empty;

            // Loop through the regions and lines of text identified by the OCR process
            foreach (var region in ocrResult.Regions)
            {
                foreach (var line in region.Lines)
                {
                    foreach (var word in line.Words)
                    {
                        extractedText += word.Text + " ";
                    }
                    extractedText += Environment.NewLine;
                }
            }

            return extractedText;
        }
    }
    
}
