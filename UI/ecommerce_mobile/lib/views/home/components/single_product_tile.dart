import 'package:ecommerce_mobile/core/components/asset_image.dart';
import 'package:ecommerce_mobile/core/components/base64_image.dart';
import 'package:ecommerce_mobile/providers/cart_provider.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../../core/constants/constants.dart';
import '../../../core/routes/app_routes.dart';
import '../../../models/product.dart';

class SingleProductTile extends StatefulWidget {
  const SingleProductTile({super.key, required this.product});

  final Product product;

  @override
  State<SingleProductTile> createState() => _SingleProductTileState();
}

class _SingleProductTileState extends State<SingleProductTile> {
  late CartProvider _cartProvider;

  @override
  void initState() {
    // TODO: implement initState
    super.initState();

    _cartProvider = context.read<CartProvider>();
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(
        horizontal: AppDefaults.padding,
        vertical: AppDefaults.padding / 2,
      ),
      child: InkWell(
        onTap: () => Navigator.pushNamed(
          context,
          AppRoutes.productDetails,
          arguments: widget.product,
        ),
        child: Column(
          children: [
            Row(
              children: [
                /// Thumbnail
                SizedBox(
                  width: 70,
                  child: AspectRatio(
                    aspectRatio: 1 / 1,
                    child: widget.product.assets.isNotEmpty
                        ? Base64ImageWithLoader(
                            widget.product.assets[0].base64Content!,
                            fit: BoxFit.contain,
                          )
                        : AssetImageWithLoader(
                            'assets/images/product_placeholder.jpg',
                            fit: BoxFit.contain,
                          ),
                  ),
                ),
                const SizedBox(width: 16),

                /// Quantity and Name
                Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Padding(
                      padding: const EdgeInsets.only(left: 8),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                           widget.product.name!.length > 20 ? "${widget.product.name!.substring(0, 20)}..." : widget.product.name!,
                            style: Theme.of(context).textTheme.bodyLarge
                                ?.copyWith(color: Colors.black),
                          ),
                          Text(
                            'Price: ${(widget.product.price!).toStringAsFixed(0)} \$',
                            style: Theme.of(context).textTheme.bodySmall,
                          ),
                        ],
                      ),
                    ),
                  ],
                ),
                const Spacer(),

                /// Price and Delete labelLarge
                Column(
                  children: [
                    IconButton(
                      constraints: const BoxConstraints(),
                      onPressed: () {
                        _cartProvider.addToCart(widget.product);
                        ScaffoldMessenger.of(context).showSnackBar(
                          const SnackBar(
                            content: Text('Product successfully added to cart'),
                          ),
                        );
                      },
                      icon: Icon(Icons.shopping_cart),
                    ),
                    const SizedBox(height: 16),
                    Text('Add to cart'),
                  ],
                ),
              ],
            ),
            const Divider(thickness: 0.1),
          ],
        ),
      ),
    );
  }
}
