terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 4.0"
    }
  }
}

provider "aws" {
  region = "us-east-1"
}

resource "random_uuid" "bucket_random_id" {
}

# Create S3 bucket to store our application source code.
resource "aws_s3_bucket" "lambda_bucket" {
  bucket        = "${random_uuid.bucket_random_id.result}-dotnet-tf-bucket"
  acl           = "private"
  force_destroy = true
}

data "archive_file" "lambda_archive" {
  type = "zip"
  source_dir  = "foursix-lambda/src/FourSix.Lambda.Authentication/bin/Release/net8.0/linux-x64/publish"
  output_path = "FourSix.Lambda.Authentication.zip"
}

resource "aws_s3_object" "lambda_bundle" {
  bucket = aws_s3_bucket.lambda_bucket.id

  key    = "FourSix.Lambda.Authentication.zip"
  source = data.archive_file.lambda_archive.output_path

  etag = filemd5(data.archive_file.lambda_archive.output_path)
}

resource "aws_iam_role" "lambda_function_role" {
  name = "FunctionIamRole_dotnet-terraform-lambda"
  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [{
      Action = "sts:AssumeRole"
      Effect = "Allow"
      Sid    = ""
      Principal = {
        Service = "lambda.amazonaws.com"
      }
      }
    ]
  })
}

resource "aws_iam_role_policy_attachment" "lambda_policy_attach" {
  role       = aws_iam_role.lambda_function_role.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole"
}

resource "aws_lambda_function" "function" {
  function_name    = "dotnet-terraform-lambda"
  s3_bucket        = aws_s3_bucket.lambda_bucket.id
  s3_key           = aws_s3_object.lambda_bundle.key
  runtime          = "dotnet6"
  handler          = "Dotnet.CDK.Lambda::Dotnet.CDK.Lambda.Function::FunctionHandler"
  source_code_hash = data.archive_file.lambda_archive.output_base64sha256
  role             = aws_iam_role.lambda_function_role.arn
  timeout          = 30
}

output "function_name" {
  value = aws_lambda_function.function.function_name
}