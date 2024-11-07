import * as sharp from "sharp";
import * as ExcelJS from "exceljs";

export async function createImageExcel(imageBuffer: Buffer, percentage = 100): Promise<Buffer> {
  try {
    console.log("Starting image processing...");

    // Resize the image
    const metadata = await sharp(imageBuffer).metadata();
    if (!metadata.width || !metadata.height) {
      throw new Error("Unable to get image dimensions");
    }

    const newWidth = Math.ceil(metadata.width * (percentage / 100));
    const newHeight = Math.ceil(metadata.height * (percentage / 100));

    console.log(`Resizing image to ${newWidth}x${newHeight}`);

    const resizedImageBuffer = await sharp(imageBuffer)
      .resize(newWidth, newHeight)
      .raw()
      .toBuffer({resolveWithObject: true});

    console.log("Image resized successfully");

    // Create a new workbook and worksheet
    const workbook = new ExcelJS.Workbook();
    const worksheet = workbook.addWorksheet("Image");

    // Set column widths and row heights
    const columnWidth = 2.75; // Adjust as needed
    const rowHeight = 15; // Adjust as needed

    worksheet.columns = Array(newWidth).fill({width: columnWidth});

    console.log("Starting to fill worksheet with color data...");

    // Fill the worksheet with color data
    for (let y = 0; y < newHeight; y++) {
      const row = worksheet.getRow(y + 1);
      row.height = rowHeight;

      for (let x = 0; x < newWidth; x++) {
        const idx = (y * newWidth + x) * 3;
        const r = resizedImageBuffer.data[idx];
        const g = resizedImageBuffer.data[idx + 1];
        const b = resizedImageBuffer.data[idx + 2];

        const cell = row.getCell(x + 1);
        cell.fill = {
          type: "pattern",
          pattern: "solid",
          fgColor: {argb: `FF${r.toString(16).padStart(2, "0")}${g.toString(16).padStart(2, "0")}${b.toString(16).padStart(2, "0")}`},
        };
      }
    }

    console.log("Worksheet filled successfully");

    // Generate Excel file
    console.log("Generating Excel file...");
    const excelBuffer = await workbook.xlsx.writeBuffer() as Buffer;
    console.log("Excel file generated successfully");

    return excelBuffer;
  } catch (error) {
    console.error("Error in createImageExcel:", error);
    throw error; // Re-throw the error to be caught by the calling function
  }
}
