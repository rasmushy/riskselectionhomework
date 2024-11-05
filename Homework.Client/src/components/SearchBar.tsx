import React from 'react';

interface SearchBarProps {
    searchTerm: string;
    setSearchTerm: React.Dispatch<React.SetStateAction<string>>;
}

/**
 * A search bar component for filtering products by title.
 * @param searchTerm - The current search term.
 * @param setSearchTerm - Function to update the search term.
 * @returns {JSX.Element} The search bar component.
 */
const SearchBar: React.FC<SearchBarProps> = ({ searchTerm, setSearchTerm }) => (
    <input
        type="text"
        placeholder="Search..."
        value={searchTerm}
        onChange={e => setSearchTerm(e.target.value)}
        className="search-bar"
    />
);

export default SearchBar;
