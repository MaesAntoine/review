<script lang="ts">
// PercentageInput.svelte
export let percentage: number;
export let isLoading: boolean;
</script>

<div class="mb-4">
  <label class="block text-gray-700 text-sm font-bold mb-2" for="percentage">
    Conversion Percentage
  </label>
  <input
    id="percentage"
    type="number"
    bind:value={percentage}
    min="1"
    max="100"
    disabled={isLoading}
    class="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
  />
</div>