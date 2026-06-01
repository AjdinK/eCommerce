import 'package:flutter/material.dart';

import '../../../core/components/product_item_square.dart';
import '../../../core/components/title_and_action_button.dart';
import '../../../core/constants/constants.dart';
import '../../../models/product_recommendation.dart';

class RecommendedProducts extends StatelessWidget {
  final List<ProductRecommendation> recommendations;

  const RecommendedProducts({
    super.key,
    required this.recommendations,
  });

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        TitleAndActionButton(
          title: 'Others also bought',
          actionLabel: '',
          onTap: () => {},
        ),
        SingleChildScrollView(
          padding: const EdgeInsets.only(left: AppDefaults.padding),
          scrollDirection: Axis.horizontal,
          child: Row(
            children: List.generate(
              recommendations.length,
              (index) => Padding(
                padding: const EdgeInsets.only(right: AppDefaults.padding),
                child: ProductItemSquare(product: recommendations[index].recommendedProduct!),
              ),
            ),
          ),
        ),
      ],
    );
  }
}