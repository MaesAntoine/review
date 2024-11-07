<script lang="ts">
// StatusMessage.svelte
export let isLoading: boolean;
</script>

{#if isLoading}
  <p class="mt-4 text-center text-gray-600">Processing... Please wait.</p>
{/if}