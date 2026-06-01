// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'product_recommendation.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ProductRecommendation _$ProductRecommendationFromJson(
  Map<String, dynamic> json,
) => ProductRecommendation(
  id: (json['id'] as num?)?.toInt(),
  productId: (json['productId'] as num?)?.toInt(),
  product: json['product'] == null
      ? null
      : Product.fromJson(json['product'] as Map<String, dynamic>),
  recommendedProductId: (json['recommendedProductId'] as num?)?.toInt(),
  recommendedProduct: json['recommendedProduct'] == null
      ? null
      : Product.fromJson(json['recommendedProduct'] as Map<String, dynamic>),
  score: (json['score'] as num?)?.toDouble(),
);

Map<String, dynamic> _$ProductRecommendationToJson(
  ProductRecommendation instance,
) => <String, dynamic>{
  'id': instance.id,
  'productId': instance.productId,
  'product': instance.product,
  'recommendedProductId': instance.recommendedProductId,
  'recommendedProduct': instance.recommendedProduct,
  'score': instance.score,
};
