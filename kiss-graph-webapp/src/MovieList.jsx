import { useEffect, useState } from "react"
import MovieListItem from "./components/MovieListItem";
import './MovieList.css';

function MovieList() {

    const [movies, setMovies] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {

        const fetchMovies = async () => {
            try {
                const response = await fetch('http://localhost:5000/api/movie');

                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }

                const data = await response.json();
                setMovies(data);
            } catch (e) {
                setError(e.message);
            } finally {
                setIsLoading(false);
            }
        };

        fetchMovies();
    }, []);

    if (isLoading) {
        return <p>Loading movies...</p>;
    }

    if (error) {
        return <p>Error fetching data: {error}</p>
    }

    return (
        <div>
            <h2>Movies</h2>
            <div className="movie-grid">
                {movies.map(movie => (
                    <MovieListItem key={movie.id} movie={movie} />
                ))}
            </div>
        </div>
    );
}

export default MovieList;