import React from 'react';

interface TrendingProductProps {
    title: string;
}

const TrendingProduct: React.FC<TrendingProductProps> = ({ title }) => (
    <div className="trending-product">
        <h2>Trending Product: {title}</h2>
    </div>
);

export default TrendingProduct;
