import 'package:ecommerce_mobile/views/home/components/recommended_products.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../core/components/app_back_button.dart';
import '../../core/components/buy_now_row_button.dart';
import '../../core/components/price_and_quantity.dart';
import '../../core/components/product_images_slider.dart';
import '../../core/components/review_row_button.dart';
import '../../core/constants/app_defaults.dart';
import '../../models/product.dart';
import '../../models/product_recommendation.dart';
import '../../providers/cart_provider.dart';
import '../../providers/product_recommendation_provider.dart';

class ProductDetailsPage extends StatefulWidget {
  const ProductDetailsPage({super.key, required this.product});

  final Product product;

  @override
  State<ProductDetailsPage> createState() => _ProductDetailsPageState();
}

class _ProductDetailsPageState extends State<ProductDetailsPage> {
  late CartProvider _cartProvider;
  late ProductRecommendationProvider _recommendationProvider;

  int _quantity = 1;

  List<ProductRecommendation> _recommendedProducts = [];

  @override
  void initState() {
    // TODO: implement initState
    super.initState();

    _cartProvider = context.read<CartProvider>();
    _recommendationProvider = context.read<ProductRecommendationProvider>();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      resizeToAvoidBottomInset: false,
      appBar: AppBar(
        leading: const AppBackButton(),
        title: const Text('Product Details'),
      ),
      bottomNavigationBar: SafeArea(
        child: Padding(
          padding: const EdgeInsets.symmetric(horizontal: AppDefaults.padding),
          child: BuyNowRow(
            onBuyButtonTap: () {},
            onCartButtonTap: () async {
              _cartProvider.addToCart(widget.product, quantity: _quantity);

              ScaffoldMessenger.of(context).showSnackBar(
                const SnackBar(
                  content: Text('Product successfully added to cart'),
                ),
              );

              try {
                var data = await _recommendationProvider
                    .getRecommendationsForProduct(widget.product.id ?? 0, 5);

                setState(() {
                  _recommendedProducts = data;
                });
                print("Recommendations: $data");
              } catch (e) {
                print("Error fetching recommendations: $e");
              }
            },
          ),
        ),
      ),
      body: SingleChildScrollView(
        child: Column(
          children: [
            ProductImagesSlider(
              images: widget.product.assets.isNotEmpty
                  ? widget.product.assets
                        .map((a) => a.base64Content ?? '')
                        .toList()
                  : ['assets/images/product_placeholder.jpg'],
            ),
            SizedBox(
              width: double.infinity,
              child: Padding(
                padding: const EdgeInsets.all(AppDefaults.padding),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      widget.product.name ?? '',
                      style: Theme.of(context).textTheme.titleLarge?.copyWith(
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                    const SizedBox(height: 8),
                    Text(
                      widget.product.weight == null
                          ? 'No wight speciifed'
                          : 'Weight: ${widget.product.weight!.toStringAsFixed(0)}',
                    ),
                  ],
                ),
              ),
            ),

            Padding(
              padding: EdgeInsets.symmetric(horizontal: AppDefaults.padding),
              child: PriceAndQuantityRow(
                currentPrice: widget.product.price ?? 0,
                orginalPrice: widget.product.price ?? 0,
                quantity: 1,
                onQuantityChanged: (value) {
                  setState(() {
                    _quantity = value;
                  });
                },
              ),
            ),
            const SizedBox(height: 8),

            /// Product Details
            Padding(
              padding: const EdgeInsets.all(AppDefaults.padding),
              child: SizedBox(
                width: double.infinity,
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      'Product Details',
                      style: Theme.of(context).textTheme.bodyLarge?.copyWith(
                        fontWeight: FontWeight.bold,
                        color: Colors.black,
                      ),
                    ),
                    const SizedBox(height: 8),
                    Text(
                      (widget.product.description != null &&
                              widget.product.description!.isNotEmpty)
                          ? widget.product.description ?? ''
                          : 'There is no product details for this product',
                    ),
                  ],
                ),
              ),
            ),

            _recommendedProducts.isNotEmpty
                ? RecommendedProducts(recommendations: _recommendedProducts)
                : SizedBox.shrink(),

            SizedBox(height: 10),

            /// Review Row
            const Padding(
              padding: EdgeInsets.symmetric(
                horizontal: AppDefaults.padding,
                // vertical: AppDefaults.padding,
              ),
              child: Column(
                children: [
                  Divider(thickness: 0.1),
                  ReviewRowButton(totalStars: 5),
                  Divider(thickness: 0.1),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}
