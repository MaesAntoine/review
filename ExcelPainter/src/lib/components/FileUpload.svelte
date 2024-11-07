<script lang="ts">
// FileUpload.svelte
export let handleFileChange: (event: Event) => void;
export let isLoading: boolean;
</script>

<div class="mb-4">
  <label class="block text-gray-700 text-sm font-bold mb-2" for="file-upload">
    Upload Image
  </label>
  <input
    id="file-upload"
    type="file"
    on:change={handleFileChange}
    accept="image/*"
    disabled={isLoading}
    class="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
  />
</div>