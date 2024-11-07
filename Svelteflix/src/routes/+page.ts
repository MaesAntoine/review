import * as api from "$lib/api";
import type { MovieDetails, MovieList } from "$lib/types";



export async function load({ fetch }: { fetch: any }) {

    const [trending, now_playing, upcoming] = await Promise.all([
        api.get(fetch, "/trending/movie/day") as Promise<MovieList>,
        api.get(fetch, "/movie/now_playing") as Promise<MovieList>,
        api.get(fetch, "/movie/upcoming") as Promise<MovieList>

    ])


    const featured = await api.get(fetch, `movie/${trending.results[0].id}`, {
        append_to_response: "images, videos, recommendations"
    }) as MovieDetails;

    return {
        trending,
        now_playing,
        upcoming,
        featured
    };
}