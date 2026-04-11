
import 'package:json_annotation/json_annotation.dart';

part 'product.g.dart';


@JsonSerializable()
class Product {
  final int? id;
  final String? name;
  final double? weight;
  final String? productState;
  final double? price;

Product({
  this.id,
  this.name,
  this.weight,
  this.productState,
  this.price
});

factory Product.fromJson(Map<String,dynamic> json) => _$ProductFromJson(json);

Map<String, dynamic> toJson() => _$ProductToJson(this);

}