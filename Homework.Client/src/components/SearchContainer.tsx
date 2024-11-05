import React from 'react';
import SearchBar from './SearchBar';
import TrendingProduct from './TrendingProduct';

interface SearchContainerProps {
    searchTerm: string;
    setSearchTerm: React.Dispatch<React.SetStateAction<string>>;
    trendingProductTitle?: string;
}

/**
 * A container component for the search bar and trending product display.
 * @param searchTerm - The current search term.
 * @param setSearchTerm - Function to update the search term.
 * @param trendingProductTitle - Title of the trending product, if available.
 * @returns {JSX.Element} The search container component.
 */
const SearchContainer: React.FC<SearchContainerProps> = ({ searchTerm, setSearchTerm, trendingProductTitle }) => (
    <div className="search-container">
        {trendingProductTitle && <TrendingProduct title={trendingProductTitle} />}
        <SearchBar searchTerm={searchTerm} setSearchTerm={setSearchTerm} />
    </div>
);

export default SearchContainer;
