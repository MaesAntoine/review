<script lang="ts">
// DownloadButton.svelte
export let handleDownload: () => void;
export let downloadUrl: string | null;
export let isLoading: boolean;
</script>

<button
  on:click={handleDownload}
  disabled={!downloadUrl || isLoading}
  class="bg-green-500 hover:bg-green-700 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline disabled:opacity-50"
>
  Download Excel
</button>