import 'package:flutter/material.dart';

void alertBox(BuildContext context, String title, String content) {
     showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: Text(title),
        content: Text(content),
        actions: [
          ElevatedButton(
            onPressed: () {
              Navigator.pop(context);
            },
            child: Text("OK"),
          ),
        ],
      ),
    );
  }


 const String mField = "This filed is mandatory";

const String numericField = "This filed is numeric";