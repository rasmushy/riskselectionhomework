import React, { useEffect, useState } from 'react';
import { fetchProducts } from '../api';
import { Product } from '../interfaces/product';
import ProductCard from './ProductCard';
import MainTitle from './MainTitle';
import SearchContainer from './SearchContainer';

/**
 * ProductGrid component displays a list of products. Including a product title, 
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
        loadAndFilterProducts();
    }, []);

    useEffect(() => {
        applySearchFilter();
    }, [searchTerm, products]);

    const loadAndFilterProducts = async () => {
        try {
            const fetchedProducts = await fetchProducts();
            const discountedProducts = filterByDiscount(fetchedProducts, 10);
            setProducts(discountedProducts);
            setFilteredProducts(discountedProducts);
            setTrendingProduct(findTrendingProduct(discountedProducts));
        } catch (error) {
            console.error("Failed to load products:", error);
        }
    };

    const applySearchFilter = () => {
        setFilteredProducts(
            products.filter(product =>
                product.title.toLowerCase().includes(searchTerm.toLowerCase())
            )
        );
    };

    /**
     * Filters products based on a minimum discount percentage.
     * @param {Product[]} products
     * @param {number} minDiscount - Minimum discount percentage.
     * @returns {Product[]}
     */
    const filterByDiscount = (products: Product[], minDiscount: number): Product[] => {
        return products.filter(product => product.discountPercentage >= minDiscount);
    };

    /**
     * Finds the trending product with the highest rating from a list of products.
     * @param {Product[]} products
     * @returns {Product | null}
     */
    const findTrendingProduct = (products: Product[]): Product | null => {
        return products.reduce((highest, product) =>
            product.rating > (highest?.rating || 0) ? product : highest,
            null as Product | null
        );
    };

    return (
        <div className="main">
            <div className="main-content">
                <MainTitle title="Products" />
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
