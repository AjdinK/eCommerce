import 'dart:convert';

import 'package:ecommerce_mobile/providers/auth_provider.dart';
import 'package:flutter/material.dart';

import 'package:http/http.dart' as http;

import '../models/product_recommendation.dart';
import '../utils/api_client_exception.dart';


class ProductRecommendationProvider with ChangeNotifier {
  String? _baseUrl;
  final String _endpoint = "ProductRecommendation";

  ProductRecommendationProvider() {
    _baseUrl = const String.fromEnvironment("baseUrl", defaultValue: "http://10.0.2.2:5126/");
  }

  Future<List<ProductRecommendation>> getRecommendationsForProduct(int productId, int numberOfRecommendations) async {
    var url = "$_baseUrl$_endpoint/GetRecommendationsForProduct?productId=$productId&numberOfRecommendations=$numberOfRecommendations";
    var uri = Uri.parse(url);
    var headers = createHeaders();

    http.Response response = await http.get(uri, headers: headers);

    validateResponse(response);

     var data = jsonDecode(response.body);

    var result = List<ProductRecommendation>.from(data.map((e) => ProductRecommendation.fromJson(e)));

    return result;
  }

}

 Map<String, String> createHeaders() {
    String accesstoken = AuthProvider.accesstoken ?? "";

    String basicAuth = "Bearer $accesstoken";

    var headers = {
      "Content-Type": "application/json",
      "Authorization": basicAuth,
    };

    return headers;
  }


void validateResponse(http.Response response) {
    if (response.statusCode < 299) {
      return;
    }
    if (response.statusCode == 401) {
      throw ApiClientException(
        'Your session has expired. Please sign in again.',
      );
    }

    final parsed = ApiErrorParser.messageFromBody(response.body);
    if (response.statusCode >= 500) {
      throw ApiClientException(
        parsed ?? 'Server error. Please try again later.',
      );
    }

    // 400 Bad Request — business rules (ClinetException), validation, etc.
    throw ApiClientException(
      parsed ?? 'Request could not be completed. Please try again.',
    );
  }
