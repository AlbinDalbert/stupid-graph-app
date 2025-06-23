import { useEffect, useState } from "react"

function AddMovieForm() {

    const [formData, setFormData] = useState ({
        title: '',
        releaseDate: ''
    });

    const handleChange = (e) => {
        const {name, value} = e.target;
        setFormData(prevFormData => ({
            ...prevFormData,
            [name]: value
        }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            const response = await fetch('http://localhost:5000/api/movie', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(formData),
            });
            
            if (!response.ok) {
                throw new Error('Failed to create movie');
            }

            const result = await response.json();
            console.log('Movie Created:' , result);
            alert('Movie added successfully!');
        } catch (err) {
            console.error('Error creating movie: ', err);
            alert('Error adding movie.');
        }
    };

    return (
        <form onSubmit={handleSubmit}>
            <h2>Add a new movie</h2>
            <div>
                <label htmlFor="title">Title</label>
                <input type="text" id="title" name="title" value={formData.title} onChange={handleChange}/>
            </div>
            <div>
                <label htmlFor="releaseDate">Release Date</label>
                <input type="text" id="releaseDate" name="releaseDate" value={formData.releaseDate} onChange={handleChange}/>
            </div>
            <button type="submit">Add Movie</button>
        </form>
    )
}

export default AddMovieForm;