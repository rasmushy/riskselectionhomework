import React from 'react';

interface TrendingProductProps {
    title: string;
}

/**
 * A component for displaying the trending product's title.
 * @param title - The trending product title to display.
 * @returns {JSX.Element} The trending product component.
 */
const TrendingProduct: React.FC<TrendingProductProps> = ({ title }) => (
    <div className="trending-product-info">
        <strong>Trending product:</strong> {title}
    </div>
);

export default TrendingProduct;
