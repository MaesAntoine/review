<script lang="ts">
	import { media } from '$lib/api';
	import type { MovieDetails, Video } from '$lib/types';

	export let movie: MovieDetails;

	$: backdrop =
		movie.images.backdrops.find((image) => !image.iso_639_1) || movie.images.backdrops[0];
</script>

<div class="movie">
	<div class="backdrop">
		<img src={media(backdrop.file_path, 1280)} alt={movie.title} />
	</div>

	<div class="info">
		<h1>{movie.title}</h1>
		<p>{movie.overview}</p>
	</div>
</div>

<style>
	.movie {
		display: grid;
		background: black;
	}

	.backdrop img {
		width: 100%;
	}

	.backdrop::after {
		content: '';
		position: absolute;
		width: 100%;
		height: 8rem;
		background: linear-gradient(to top, black, transparent);
		left: 0;
		bottom: 0;
	}

	.info {
		display: flex;
		flex-direction: column;
		padding: 0 var(--side);
		gap: 1rem;
		margin-top: -4rem;
	}

	.info h1,
	.info p {
		margin: 0;
	}

	@media (min-width: 60rem) {
		.movie {
			grid-template-columns: 1fr 60rem;
			grid-template-rows: auto;
		}

		.backdrop {
			grid-column: 2/4;
		}

		.backdrop::after {
			width: 30rem;
			height: 100%;
			left: 0;
			bottom: 0;
			background: linear-gradient(to right, black, transparent);
			filter: drop-shadow(0 0 2em black);
		}

		.info {
			position: absolute;
			justify-content: center;
			top: 0;
			left: 0;
			width: 40em;
			height: 100%;
			grid-column: 1 2;
			margin-top: 0;
		}
	}
</style>
