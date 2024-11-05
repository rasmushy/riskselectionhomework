import React, { useEffect, useState } from 'react';
import { fetchProducts } from '../api';
import { Product } from '../interfaces/product';
import ProductCard from './ProductCard';
import HeaderTitle from './HeaderTitle';
import SearchContainer from './SearchContainer';

/**
 * ProductGrid component displays a list of products, including a header, 
 * search functionality, and a trending product indicator.
 *
 * @component
 * @returns {JSX.Element} The rendered ProductGrid component.
 */
const ProductGrid: React.FC = () => {
    const [products, setProducts] = useState<Product[]>([]);
    const [filteredProducts, setFilteredProducts] = useState<Product[]>([]);
    const [trendingProduct, setTrendingProduct] = useState<Product | null>(null);
    const [searchTerm, setSearchTerm] = useState('');

    useEffect(() => {
        const loadProducts = async () => {
            try {
                const products = await fetchProducts();
                setProducts(products);
                setFilteredProducts(products);

                const highestRatedProduct = products.reduce((highest, product) =>
                    product.rating > (highest?.rating || 0) ? product : highest,
                    null as Product | null
                );
                setTrendingProduct(highestRatedProduct);
            } catch (error) {
                console.error("Failed to load products:", error);
            }
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
            <div className="main-content">
                <HeaderTitle title="Products" />
                <SearchContainer 
                    searchTerm={searchTerm} 
                    setSearchTerm={setSearchTerm} 
                    trendingProductTitle={trendingProduct?.title || ''} 
                />
                <div className="product-grid">
                    {filteredProducts.map(product => (
                        <ProductCard key={product.id} product={product} />
                    ))}
                </div>
            </div>
        </div>
    );
};

export default ProductGrid;
