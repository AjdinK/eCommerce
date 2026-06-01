import 'package:ecommerce_mobile/models/product.dart';
import 'package:json_annotation/json_annotation.dart';

part 'product_recommendation.g.dart';

@JsonSerializable()
class ProductRecommendation {
  final int? id;
  final int? productId;
  final Product? product;
  final int? recommendedProductId;
  final Product? recommendedProduct;
  final double? score;

  ProductRecommendation({
    this.id,
    this.productId,
    this.product,
    this.recommendedProductId,
    this.recommendedProduct,
    this.score
  });

  factory ProductRecommendation.fromJson(Map<String, dynamic> json) =>
      _$ProductRecommendationFromJson(json);

  Map<String, dynamic> toJson() => _$ProductRecommendationToJson(this);
}