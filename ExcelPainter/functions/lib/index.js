"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.convertImageToExcel = void 0;
const functions = require("firebase-functions");
const admin = require("firebase-admin");
const imageToExcel_1 = require("./imageToExcel");
const cors = require("cors");
const fs = require("fs");
const path = require("path");
const serviceAccountPath = path.join(__dirname, '..', 'service-account.json');
const serviceAccount = JSON.parse(fs.readFileSync(serviceAccountPath, 'utf8'));
admin.initializeApp({
    credential: admin.credential.cert(serviceAccount),
    storageBucket: 'excelpainter-85f02.appspot.com' // replace with your actual bucket name
});
const corsHandler = cors({ origin: true });
exports.convertImageToExcel = functions.https.onRequest((request, response) => {
    corsHandler(request, response, async () => {
        try {
            console.log("Request body:", JSON.stringify(request.body));
            if (request.method !== "POST") {
                response.status(405).send("Method Not Allowed");
                return;
            }
            const { image, percentage } = request.body;
            if (!image) {
                console.error("No image data provided");
                response.status(400).send("No image data provided.");
                return;
            }
            console.log(`Received image data of length: ${image.length}`);
            console.log(`Received percentage: ${percentage}`);
            const imageBuffer = Buffer.from(image, "base64");
            const parsedPercentage = parseInt(percentage) || 100;
            console.log("Creating Excel file...");
            const excelBuffer = await (0, imageToExcel_1.createImageExcel)(imageBuffer, parsedPercentage);
            console.log("Excel file created successfully");
            const excelFileName = `converted_${Date.now()}.xlsx`;
            const bucket = admin.storage().bucket();
            const excelFile = bucket.file(excelFileName);
            console.log("Saving Excel file to bucket...");
            await excelFile.save(excelBuffer);
            console.log("Excel file saved to bucket successfully");
            console.log("Generating signed URL...");
            const [url] = await excelFile.getSignedUrl({
                action: "read",
                expires: Date.now() + 60 * 60 * 1000,
            });
            console.log("Signed URL generated successfully");
            response.status(200).json({ downloadUrl: url });
        }
        catch (error) {
            console.error("Error in convertImageToExcel:", error);
            response.status(500).json({
                error: "An error occurred during processing",
                message: error instanceof Error ? error.message : "Unknown error",
                stack: error instanceof Error ? error.stack : "No stack trace",
            });
        }
    });
});
//# sourceMappingURL=index.js.map