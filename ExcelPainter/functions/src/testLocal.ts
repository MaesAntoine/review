import * as fs from "fs";
import * as path from "path";
import {createImageExcel} from "./imageToExcel";

async function testCreateImageExcel(): Promise<void> {
  try {
    // Read an image file
    const imagePath = path.join(__dirname, "Original_Doge_meme.jpg"); // Make sure this file exists
    const imageBuffer = fs.readFileSync(imagePath);

    // Call the function
    const excelBuffer = await createImageExcel(imageBuffer, 50); // 50% as an example

    // Write the result to a file
    fs.writeFileSync("output.xlsx", excelBuffer);
    console.log("Excel file created successfully: output.xlsx");
  } catch (error) {
    console.error("Error:", error);
  }
}

testCreateImageExcel();
