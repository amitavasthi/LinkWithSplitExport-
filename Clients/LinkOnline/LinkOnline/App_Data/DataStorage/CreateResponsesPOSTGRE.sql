CREATE TABLE "resp"."Var_{0}"(
	"Id" "uuid" NOT NULL,
	"IdRespondent" "uuid" NOT NULL,
	"IdStudy" "uuid" NOT NULL,
	"IdCategory" "uuid" NULL,
	"NumericAnswer" numeric NULL,
	"TextAnswer" "text" NULL)