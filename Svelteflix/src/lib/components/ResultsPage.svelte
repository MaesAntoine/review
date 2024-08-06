<script lang="ts">
	import { media } from '$lib/api';
	import type { MovieListResult } from '$lib/types';
	import { createEventDispatcher, onMount } from 'svelte';

	export let movies: MovieListResult[];
	export let next: string | null;

	let dispatch = createEventDispatcher();

	let viewport: HTMLDivElement;
	let results: HTMLDivElement;
	let item_width: number;
	let item_height: number;
	let num_columns: number = 4;

	let a: number = 0; // first visible element
	let b: number = movies.length; // first invisible element
	let padding_top: number = 0;
	let padding_bottom: number = 0;

	function handle_resize() {
		const first = results.firstChild! as HTMLAnchorElement; // works because binded to html div. ! means we guarantee to have something else than null.

		item_width = first.offsetWidth;
		item_height = first.offsetHeight;

		num_columns = 4;

		handle_scroll();
	}

	function handle_scroll() {
		// checks which elements are in view and tell the parent how are we close to the end to fetch more.
		const { scrollHeight, scrollTop, clientHeight } = viewport;

		const remaining = scrollHeight - (scrollTop + clientHeight);
		if (remaining < 400) {
			dispatch('end');
		}

		a = Math.floor(scrollTop / item_height) * num_columns;
		b = Math.ceil((scrollTop + clientHeight) / item_height) * num_columns;

		padding_top = Math.floor(a / num_columns) * item_height;
		padding_bottom = Math.floor((movies.length - b) / num_columns) * item_height;
	}

	onMount(handle_resize);
</script>

<svelte:window on:resize={handle_resize} />

<div bind:this={viewport} class="viewport" on:scroll={handle_scroll}>
	<div
		bind:this={results}
		class="results"
		style:padding-top="{padding_top}px"
		style:padding-bottom="{padding_bottom}px"
	>
		{#each movies.slice(a, b) as movie}
			<a href="/movies/{movie.id}">
				<img src={media(movie.poster_path, 500)} alt={movie.title} />
			</a>
		{/each}
	</div>
	{#if next}
		<a href={next}>Next page</a>
	{/if}
</div>

<style>
	.viewport {
		overflow-y: auto;
	}

	.viewport::-webkit-scrollbar {
		display: none;
	}

	.results {
		display: grid;
		grid-template-columns: repeat(4, 1fr);
		width: 100%;
	}

	a {
		width: 100%;
		height: auto;
		aspect-ratio: 2/3;
		padding: 0.5rem;
	}

	img {
		width: 100%;
	}
</style>
