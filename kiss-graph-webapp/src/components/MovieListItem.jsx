import React from "react";
import './MovieListItem.css';
import placeholderImage from '../assets/ImageNotFound.png'

function MovieListItem({movie}) {
    const API_BASE_URL = 'http://localhost:5000';
    const fullImageUrl = movie.imageUrl ? `${API_BASE_URL}${movie.imageUrl}` : placeholderImage;



    return (
        <div className="movie-card">
            <div className="movie-card-image-wrapper">
                <img src={fullImageUrl} alt={`Posrter for ${movie.title}`} className="movie-card-image" />
            </div>
            <div className="movie-card-content">
                <h3 className="movie-card-title">{movie.title}</h3>
                <p className="movie-card-release-date">{movie.releaseDate}</p>
            </div>
        </div>
    )
}

export default MovieListItem;