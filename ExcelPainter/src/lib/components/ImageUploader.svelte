<script lang="ts">
import { functions } from '$lib/firebase';

let file: File | null = null;
let percentage: number = 100;
let downloadUrl: string | null = null;
let isLoading: boolean = false;

async function handleSubmit() {
  if (!file) return;
  isLoading = true;
  downloadUrl = null;

  const reader = new FileReader();
  reader.onload = async function(e: ProgressEvent<FileReader>) {
    const result = e.target?.result;
    if (typeof result !== 'string') return;
    
    const imageData = result.split(',')[1]; // Get base64 data

    try {
      const response = await fetch(`${import.meta.env.VITE_FIREBASE_FUNCTIONS_URL}/excelpainter-85f02/us-central1/convertImageToExcel`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ image: imageData, percentage }),
      });

      if (!response.ok) {
        const errorData = await response.json();
        console.error('Full error response:', errorData);
        throw new Error(`HTTP error! status: ${response.status}, message: ${JSON.stringify(errorData)}`);
      }

      const data = await response.json();
      console.log('Excel file URL:', data.downloadUrl);
      downloadUrl = data.downloadUrl;
    } catch (error) {
      console.error('Error:', error);
      alert('An error occurred while processing the image.');
    } finally {
      isLoading = false;
    }
  };
  reader.readAsDataURL(file);
}

function handleFileChange(event: Event) {
  const target = event.target as HTMLInputElement;
  file = target.files ? target.files[0] : null;
  downloadUrl = null; // Reset download URL when a new file is selected
}

function handleDownload() {
  if (downloadUrl) {
    window.open(downloadUrl, '_blank');
  }
}
</script>

<input type="file" on:change={handleFileChange} accept="image/*" disabled={isLoading}>
<input type="number" bind:value={percentage} min="1" max="100" disabled={isLoading}>
<button on:click={handleSubmit} disabled={!file || isLoading}>
  {isLoading ? 'Converting...' : 'Convert to Excel'}
</button>
<button on:click={handleDownload} disabled={!downloadUrl || isLoading}>
  Download Excel
</button>

{#if isLoading}
  <p>Processing... Please wait.</p>
{/if}

<style>
  button {
    margin-top: 10px;
    margin-right: 10px;
  }
  input {
    margin-bottom: 10px;
  }
</style>