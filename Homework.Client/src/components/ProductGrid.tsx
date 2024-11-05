import React, { useEffect, useState } from 'react';
import { fetchProducts } from '../api';
import { Product } from '../interfaces/product';
import ProductCard from './ProductCard';
import MainTitle from './MainTitle';
import SearchContainer from './SearchContainer';

const MIN_DISCOUNT_PERCENTAGE = 10;

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
            const validProducts = filterValidProducts(fetchedProducts);
            setProducts(validProducts);
            setFilteredProducts(validProducts);
            setTrendingProduct(findTrendingProduct(validProducts));
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
     * Filters products based on a minimum discount percentage and checks for non-empty brand and title.
     * @param {Product[]} products
     * @returns {Product[]}
     */
    const filterValidProducts = (products: Product[]): Product[] => {
        return products.filter(
            product =>
                product.discountPercentage >= MIN_DISCOUNT_PERCENTAGE &&
                product.brand &&
                product.title
        );
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
