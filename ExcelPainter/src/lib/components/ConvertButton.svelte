<script lang="ts">
// ConvertButton.svelte
export let handleSubmit: () => void;
export let file: File | null;
export let isLoading: boolean;
</script>

<button
  on:click={handleSubmit}
  disabled={!file || isLoading}
  class="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline disabled:opacity-50"
>
  {isLoading ? 'Converting...' : 'Convert to Excel'}
</button>