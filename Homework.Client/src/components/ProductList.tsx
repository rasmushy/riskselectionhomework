import React, { useEffect, useState } from 'react';
import { fetchProducts } from '../api';
import { Product } from '../interfaces/product';
import ProductCard from './ProductCard';
import TrendingProduct from './TrendingProduct';
import SearchBar from './SearchBar';

const ProductList: React.FC = () => {
    const [products, setProducts] = useState<Product[]>([]);
    const [filteredProducts, setFilteredProducts] = useState<Product[]>([]);
    const [trendingProduct, setTrendingProduct] = useState<Product | null>(null);
    const [searchTerm, setSearchTerm] = useState('');

    useEffect(() => {
        const loadProducts = async () => {
            const products = await fetchProducts();
            setProducts(products);
            setFilteredProducts(products);

            const trendingProduct = products.reduce((highestRated, product) => 
                product.rating > (highestRated?.rating || 0) ? product : highestRated
            , null);

            setTrendingProduct(trendingProduct);
        };
        loadProducts();
    }, []);

    useEffect(() => {
        setFilteredProducts(
            products.filter(product =>
                product.title.toLowerCase().includes(searchTerm.toLowerCase())
            )
        );
    }, [searchTerm, products]);

    return (
        <div className="main">
            <h2 className="products-title">Products</h2>
                <div className="search-bar">
                {trendingProduct && (
                    <div className="trending-product">
                        <strong>Trending product:</strong> {trendingProduct.title}
                    </div>
                )}
                    <SearchBar searchTerm={searchTerm} setSearchTerm={setSearchTerm} />
                </div>
            <div className="product-grid">
                {filteredProducts.map(product => (
                    <ProductCard key={product.id} product={product} />
                ))}
            </div>
        </div>
    );
};

export default ProductList;
