<script lang="ts">
import Header from '$lib/components/Header.svelte';
import FileUpload from '$lib/components/FileUpload.svelte';
import PercentageInput from '$lib/components/PercentageInput.svelte';
import ConvertButton from '$lib/components/ConvertButton.svelte';
import DownloadButton from '$lib/components/DownloadButton.svelte';
import StatusMessage from '$lib/components/StatusMessage.svelte';

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

<main class="container mx-auto px-4 py-8">
  <Header />
  
  <div class="bg-white shadow-md rounded px-8 pt-6 pb-8 mb-4">
    <FileUpload {handleFileChange} {isLoading} />
    <PercentageInput bind:percentage {isLoading} />
    <div class="flex justify-between mt-6">
      <ConvertButton {handleSubmit} {file} {isLoading} />
      <DownloadButton {handleDownload} {downloadUrl} {isLoading} />
    </div>
    <StatusMessage {isLoading} />
  </div>
</main>

<style>
  :global(body) {
    background-color: #f0f4f8;
    font-family: Arial, sans-serif;
  }
</style>