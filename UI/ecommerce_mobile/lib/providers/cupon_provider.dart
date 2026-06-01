import 'dart:convert';

import 'package:ecommerce_mobile/models/cupon.dart';
import 'package:ecommerce_mobile/providers/base_provider.dart';
import 'package:http/http.dart' as http;

class CuponProvider extends BaseProvider<Cupon> {
  CuponProvider() : super("Cupons");

  @override
  Cupon fromJson(data) {
    return Cupon.fromJson(data);
  }

  Future<void> toggleActivity(int id) async {
    var url = "${BaseProvider.baseUrl}$endpoint/$id/ToggleActivity";

    var uri = Uri.parse(url);

    var headers = createHeaders();

    http.Response response = await http.put(uri, headers: headers);

    validateResponse(response);
  }

  Future<Cupon> getByCode(String code) async {
    var url = "${BaseProvider.baseUrl}$endpoint/GetByCode?code=$code";

    var uri = Uri.parse(url);
    var headers = createHeaders();

    http.Response response = await http.get(uri, headers: headers);

    validateResponse(response);
    var data = jsonDecode(response.body);

    return fromJson(data);
  }
}
