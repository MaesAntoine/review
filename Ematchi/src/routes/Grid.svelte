<script lang="ts">
	import { createEventDispatcher } from 'svelte';
	import Square from './Square.svelte';

	export let grid: string[];
	export let found: string[];

	const dispatch = createEventDispatcher();

	let a = -1;
	let b = -1;
	let reset_timeout: number;
</script>

<div class="grid">
	{#each grid as emoji, i}
		<Square
			{emoji}
			on:click={() => {
				clearTimeout(reset_timeout);

				// nothing is selected
				if (a < 0 && b < 0) {
					a = i; // then card_a is selected
				} else if (b < 0) {
					b = i;
					// card_b not selected
					if (grid[a] === grid[b]) {
						// correct
						dispatch('found', {
							emoji
						});
					} else {
						reset_timeout = setTimeout(() => {
							a = b = -1;
						}, 1000);
					}
				} else {
					b = -1;
					a = i; // a becomes new selection
				}
			}}
			selected={a === i || b === i}
			found={found.includes(emoji)}
			group={grid.indexOf(emoji) === i ? 'a' : 'b'}
		/>
	{/each}
</div>

<style>
	.grid {
		display: grid;
		grid-template-columns: repeat(var(--size), 1fr);
		grid-template-rows: repeat(var(--size), 1fr);
		grid-gap: 0.4em;
		height: 100%;
		perspective: 100vh;
	}
</style>
