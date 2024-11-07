"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const fs = require("fs");
const path = require("path");
const imageToExcel_1 = require("./imageToExcel");
async function testCreateImageExcel() {
    try {
        // Read an image file
        const imagePath = path.join(__dirname, "Original_Doge_meme.jpg"); // Make sure this file exists
        const imageBuffer = fs.readFileSync(imagePath);
        // Call the function
        const excelBuffer = await (0, imageToExcel_1.createImageExcel)(imageBuffer, 50); // 50% as an example
        // Write the result to a file
        fs.writeFileSync("output.xlsx", excelBuffer);
        console.log("Excel file created successfully: output.xlsx");
    }
    catch (error) {
        console.error("Error:", error);
    }
}
testCreateImageExcel();
//# sourceMappingURL=testLocal.js.map